package com.arcane.arcana.common.config;

import com.arcane.arcana.common.filter.ApiKeyAuthFilter;
import com.arcane.arcana.common.filter.JWTTokenValidatorFilter;
import com.arcane.arcana.common.handler.CustomAuthenticationSuccessHandler;
import com.arcane.arcana.common.handler.CustomLoginFailureHandler;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.user.repository.UserRepository;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;  // 추가됨
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

/**
 * 프로젝트의 보안 설정을 구성
 */
@Configuration
@EnableWebSecurity
public class ProjectSecurityConfig {

    @Value("${api.key}")
    private String apiKey;

    private final JwtUtil jwtUtil;
    private final UserRepository userRepository;

    public ProjectSecurityConfig(JwtUtil jwtUtil, UserRepository userRepository) {
        this.jwtUtil = jwtUtil;
        this.userRepository = userRepository;
    }

    /**
     * 커스텀 인증 성공 핸들러 빈 생성
     */
    @Bean
    public CustomAuthenticationSuccessHandler customAuthenticationSuccessHandler() {
        return new CustomAuthenticationSuccessHandler(jwtUtil, userRepository);
    }

    /**
     * 커스텀 로그인 실패 핸들러 빈 생성
     */
    @Bean
    public CustomLoginFailureHandler customLoginFailureHandler() {
        return new CustomLoginFailureHandler();
    }

    /**
     * JWT 토큰 검증 필터 빈 생성
     */
    @Bean
    public JWTTokenValidatorFilter jwtTokenValidatorFilter() {
        return new JWTTokenValidatorFilter(jwtUtil, userRepository);
    }

    /**
     * API 키 인증 필터 빈 생성
     */
    @Bean
    public ApiKeyAuthFilter apiKeyAuthFilter() {
        return new ApiKeyAuthFilter(apiKey);
    }

    /**
     * 보안 필터 체인 설정
     */
    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        http
            .sessionManagement(session -> session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .csrf(AbstractHttpConfigurer::disable)
            .addFilterBefore(jwtTokenValidatorFilter(), UsernamePasswordAuthenticationFilter.class)
            .addFilterBefore(apiKeyAuthFilter(), JWTTokenValidatorFilter.class)
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/public/**", "/user/login", "/user/verify-email", "/health", "/swagger-ui/**", "/v3/api-docs/**", "/user/register").permitAll()
                .anyRequest().authenticated()
            );
        return http.build();
    }

    /**
     * 비밀번호 암호화를 위한 BCryptPasswordEncoder 빈 생성
     */
    @Bean
    public BCryptPasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}
