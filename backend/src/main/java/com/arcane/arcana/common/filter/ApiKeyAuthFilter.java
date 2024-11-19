package com.arcane.arcana.common.filter;

import com.arcane.arcana.common.service.RedisService;
import org.springframework.security.core.context.SecurityContextHolder;
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
 * API 키 인증을 위한 필터
 */
public class ApiKeyAuthFilter extends OncePerRequestFilter {

    private final String apiKey;
    private final RedisService redisService;

    public ApiKeyAuthFilter(String apiKey, RedisService redisService) {
        this.apiKey = apiKey;
        this.redisService = redisService;
    }

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response,
        FilterChain filterChain)
        throws ServletException, IOException {
        String requestApiKey = request.getHeader("SIGNALING-API-KEY");
        if (apiKey.equals(requestApiKey)) {
            UsernamePasswordAuthenticationToken authentication = new UsernamePasswordAuthenticationToken(
                "SIGNALING", null,
                Collections.singletonList(new SimpleGrantedAuthority("ROLE_SIGNAL")));
            SecurityContextHolder.getContext().setAuthentication(authentication);
            filterChain.doFilter(request, response);
        } else if (requestApiKey != null) { // API 키가 요청에 존재하지만 유효하지 않을 경우
            response.setStatus(HttpServletResponse.SC_FORBIDDEN);
            response.getWriter().write("{\"message\": \"유효하지 않은 API 키입니다.\", \"data\": null}");
        } else { // API 키가 요청에 존재하지 않을 경우
            filterChain.doFilter(request, response);
        }
    }
}
