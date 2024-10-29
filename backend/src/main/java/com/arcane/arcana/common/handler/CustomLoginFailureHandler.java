package com.arcane.arcana.common.handler;

import com.arcane.arcana.common.dto.ApiResponse;
import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.web.authentication.AuthenticationFailureHandler;
import org.springframework.stereotype.Component;

import java.io.IOException;

/**
 * 인증 실패 시 동작을 정의
 */
@Component
public class CustomLoginFailureHandler implements AuthenticationFailureHandler {

    @Override
    public void onAuthenticationFailure(HttpServletRequest request, HttpServletResponse response,
        AuthenticationException exception) throws IOException, ServletException {

        response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
        response.setContentType("application/json;charset=UTF-8");

        ApiResponse<String> apiResponse = new ApiResponse<>("로그인 실패", null);

        ObjectMapper objectMapper = new ObjectMapper();
        String jsonResponse = objectMapper.writeValueAsString(apiResponse);

        response.getWriter().write(jsonResponse);
        response.getWriter().flush();
    }
}
