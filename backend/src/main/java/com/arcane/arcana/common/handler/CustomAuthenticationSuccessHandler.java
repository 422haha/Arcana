package com.arcane.arcana.common.handler;

import com.arcane.arcana.common.dto.ApiResponse;
import com.arcane.arcana.user.dto.LoginResponseDto;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.user.dto.CustomUserDetails;
import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.security.core.Authentication;
import org.springframework.security.web.authentication.AuthenticationSuccessHandler;
import org.springframework.stereotype.Component;

import java.io.IOException;

/**
 * 인증 성공 시 동작을 정의
 */
@Component
public class CustomAuthenticationSuccessHandler implements AuthenticationSuccessHandler {

    private final JwtUtil jwtUtil;
    private final RedisService redisService;

    public CustomAuthenticationSuccessHandler(JwtUtil jwtUtil, RedisService redisService) {
        this.jwtUtil = jwtUtil;
        this.redisService = redisService;
    }

    @Override
    public void onAuthenticationSuccess(HttpServletRequest request, HttpServletResponse response,
        Authentication authentication) throws IOException, ServletException {

        String email = authentication.getName();

        Object principal = authentication.getPrincipal();
        if (!(principal instanceof CustomUserDetails)) {
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            response.getWriter().write("{\"message\": \"사용자 정보를 찾을 수 없습니다.\", \"data\": null}");
            return;
        }

        CustomUserDetails userDetails = (CustomUserDetails) principal;

        String accessToken = jwtUtil.generateAccessToken(email);
        String refreshToken = jwtUtil.generateRefreshToken(email);

        // Redis에 Refresh Token 저장
        redisService.setStringValue("refresh_token:" + email, refreshToken,
            jwtUtil.getRefreshTokenExpirationMinutes());

        response.setContentType("application/json;charset=UTF-8");

        LoginResponseDto loginResponseDto = new LoginResponseDto(
            accessToken,
            refreshToken,
            userDetails.getId(),
            userDetails.getNickname(),
            userDetails.getLanguage(),
            userDetails.getMoney(),
            userDetails.getHealth()
        );

        ApiResponse<LoginResponseDto> apiResponse = new ApiResponse<>("로그인 성공", loginResponseDto);

        ObjectMapper objectMapper = new ObjectMapper();
        String jsonResponse = objectMapper.writeValueAsString(apiResponse);

        response.getWriter().write(jsonResponse);
        response.getWriter().flush();
    }
}
