package com.arcane.arcana.common.config;

import com.arcane.arcana.common.filter.ApiKeyAuthFilter;
import com.arcane.arcana.common.filter.JWTTokenValidatorFilter;
import com.arcane.arcana.common.handler.CustomAuthenticationSuccessHandler;
import com.arcane.arcana.common.handler.CustomLoginFailureHandler;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.user.service.CustomUserDetailsService;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.AuthenticationProvider;
import org.springframework.security.authentication.dao.DaoAuthenticationProvider;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;

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
    private final CustomUserDetailsService userDetailsService;
    private final RedisTemplate<String, Object> redisTemplate;

    public ProjectSecurityConfig(JwtUtil jwtUtil, UserRepository userRepository,
        CustomUserDetailsService userDetailsService, RedisTemplate<String, Object> redisTemplate) {
        this.jwtUtil = jwtUtil;
        this.userRepository = userRepository;
        this.userDetailsService = userDetailsService;
        this.redisTemplate = redisTemplate;
    }

    /**
     * 비밀번호 암호화를 위한 BCryptPasswordEncoder 빈 생성
     */
    @Bean
    public BCryptPasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    /**
     * AuthenticationProvider를 정의
     */
    @Bean
    public AuthenticationProvider authenticationProvider() {
        DaoAuthenticationProvider authProvider = new DaoAuthenticationProvider();
        authProvider.setUserDetailsService(userDetailsService);
        authProvider.setPasswordEncoder(passwordEncoder());
        return authProvider;
    }

    /**
     * AuthenticationManager를 정의
     */
    @Bean
    public AuthenticationManager authenticationManager(
        AuthenticationConfiguration authenticationConfiguration) throws Exception {
        return authenticationConfiguration.getAuthenticationManager();
    }

    /**
     * 보안 필터 체인 설정
     */
    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        http
            .sessionManagement(
                session -> session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
            .csrf(csrf -> csrf.disable())
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/public/**", "/user/register", "/user/login",
                    "/user/verify-email", "/user/register/authnumber", "/user/forgot-password",
                    "/user/reset-password", "/user/reset-password**", "/user/reset-password/**",
                    "/health", "/swagger-ui/**", "/v3/api-docs/**",
                    "/subscribe", "/user/check-nickname", "/token/refresh")
                .permitAll()
                .anyRequest().authenticated()
            )
            .authenticationProvider(authenticationProvider())
            .addFilterBefore(jwtTokenValidatorFilter(),
                org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter.class)
            .addFilterBefore(apiKeyAuthFilter(), JWTTokenValidatorFilter.class);

        return http.build();
    }

    /**
     * 커스텀 인증 성공 핸들러 빈 생성
     */
    @Bean
    public CustomAuthenticationSuccessHandler customAuthenticationSuccessHandler() {
        return new CustomAuthenticationSuccessHandler(jwtUtil, userRepository, redisService());
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
        return new JWTTokenValidatorFilter(jwtUtil, redisService());
    }

    /**
     * API 키 인증 필터 빈 생성
     */
    @Bean
    public ApiKeyAuthFilter apiKeyAuthFilter() {
        return new ApiKeyAuthFilter(apiKey, redisService());
    }

    /**
     * RedisService 빈 생성
     */
    @Bean
    public RedisService redisService() {
        return new RedisService(redisTemplate);
    }
}
