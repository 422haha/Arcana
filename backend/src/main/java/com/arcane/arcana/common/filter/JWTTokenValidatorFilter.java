package com.arcane.arcana.common.filter;

import com.arcane.arcana.common.constants.SecurityConstants;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.util.JwtUtil;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.Authentication;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.web.filter.OncePerRequestFilter;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import java.io.IOException;
import java.util.Collections;

/**
 * JWT 토큰을 검증하는 필터
 */
public class JWTTokenValidatorFilter extends OncePerRequestFilter {

    private final JwtUtil jwtUtil;
    private final RedisService redisService;

    public JWTTokenValidatorFilter(JwtUtil jwtUtil, RedisService redisService) {
        this.jwtUtil = jwtUtil;
        this.redisService = redisService;
    }

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response,
        FilterChain filterChain)
        throws ServletException, IOException {
        String jwt = request.getHeader(SecurityConstants.JWT_HEADER);
        if (jwt != null && jwt.startsWith("Bearer ")) {
            String token = jwt.substring(7);
            if (jwtUtil.isTokenValid(token)) {
                // 토큰이 블랙리스트에 있는지 확인
                if (redisService.isTokenBlacklisted(token)) {
                    response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
                    response.getWriter().write("{\"message\": \"로그아웃된 토큰입니다.\", \"data\": null}");
                    return;
                }
                String email = jwtUtil.getEmailFromToken(token);
                UsernamePasswordAuthenticationToken authentication = new UsernamePasswordAuthenticationToken(
                    email, null,
                    Collections.singletonList(new SimpleGrantedAuthority("ROLE_USER")));
                SecurityContextHolder.getContext().setAuthentication(authentication);
            } else {
                response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
                response.getWriter().write("{\"message\": \"유효하지 않은 토큰입니다.\", \"data\": null}");
                return;
            }
        }
        filterChain.doFilter(request, response);
    }
}
