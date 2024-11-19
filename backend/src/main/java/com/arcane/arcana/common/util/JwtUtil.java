package com.arcane.arcana.common.util;

import com.arcane.arcana.common.exception.CustomException;
import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.repository.UserRepository;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.security.Keys;
import io.jsonwebtoken.io.Decoders;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.security.Key;
import java.util.Date;
import java.util.Optional;

/**
 * JWT 토큰 생성을 담당
 */
@Component
public class JwtUtil {

    private static final Logger logger = LoggerFactory.getLogger(JwtUtil.class);

    private final UserRepository userRepository;

    @Value("${jwt.secret}")
    private String secret;

    @Value("${jwt.accessTokenExpiration}")
    private long accessTokenExpiration;

    @Value("${jwt.refreshTokenExpiration}")
    private long refreshTokenExpiration;

    public JwtUtil(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    /**
     * 서명 키를 생성
     */
    private Key getSigningKey() {
        byte[] keyBytes = Decoders.BASE64.decode(secret);
        return Keys.hmacShaKeyFor(keyBytes);
    }

    /**
     * 액세스 토큰을 생성
     */
    public String generateAccessToken(String email) {
        return Jwts.builder()
            .setSubject(email)
            .setIssuedAt(new Date())
            .setExpiration(new Date(System.currentTimeMillis() + accessTokenExpiration))
            .signWith(getSigningKey(), SignatureAlgorithm.HS256)
            .compact();
    }

    /**
     * 리프레시 토큰을 생성
     */
    public String generateRefreshToken(String email) {
        return Jwts.builder()
            .setSubject(email)
            .setIssuedAt(new Date())
            .setExpiration(new Date(System.currentTimeMillis() + refreshTokenExpiration))
            .signWith(getSigningKey(), SignatureAlgorithm.HS256)
            .compact();
    }

    /**
     * 토큰의 유효성을 검사
     */
    public boolean isTokenValid(String token) {
        try {
            Jwts.parserBuilder()
                .setSigningKey(getSigningKey())
                .build()
                .parseClaimsJws(token);
            return true;
        } catch (Exception e) {
            logger.error("유효하지 않은 JWT 토큰: {}", e.getMessage());
            return false;
        }
    }

    /**
     * 토큰에서 이메일을 추출
     */
    public String getEmailFromToken(String token) {
        Claims claims = Jwts.parserBuilder()
            .setSigningKey(getSigningKey())
            .build()
            .parseClaimsJws(token)
            .getBody();
        return claims.getSubject();
    }

    /**
     * 토큰에서 사용자 ID를 추출
     */
    public long getUserIdFromToken(String bearerToken) {
        String token = extractTokenFromHeader(bearerToken);
        String email = getEmailFromToken(token);

        return userRepository.findByEmail(email)
            .map(User::getId)
            .orElseThrow(
                () -> new CustomException("해당 이메일의 사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));
    }

    /**
     * Refresh Token 유효 기간을 분 단위로 반환
     */
    public long getRefreshTokenExpirationMinutes() {
        return refreshTokenExpiration / 1000 / 60;
    }

    /**
     * 이메일 인증 토큰 생성
     */
    public String generateEmailVerificationToken(String email) {
        return Jwts.builder()
            .setSubject(email)
            .setIssuedAt(new Date())
            .setExpiration(new Date(System.currentTimeMillis() + 3600000)) // 1시간 유효
            .signWith(getSigningKey(), SignatureAlgorithm.HS256)
            .compact();
    }

    /**
     * 비밀번호 재설정 토큰 생성
     */
    public String generatePasswordResetToken(String email) {
        return Jwts.builder()
            .setSubject(email)
            .setIssuedAt(new Date())
            .setExpiration(new Date(System.currentTimeMillis() + 3600000)) // 1시간 유효
            .signWith(getSigningKey(), SignatureAlgorithm.HS256)
            .compact();
    }

    /**
     * 요청에서 이메일 추출
     */
    public String extractEmailFromRequest(HttpServletRequest request) {
        String authHeader = request.getHeader("Authorization");
        String token = extractTokenFromHeader(authHeader);
        if (!isTokenValid(token)) {
            throw new CustomException("유효하지 않은 토큰입니다.", HttpStatus.UNAUTHORIZED);
        }
        return getEmailFromToken(token);
    }

    /**
     * 인증 헤더에서 토큰 추출
     */
    public String extractTokenFromHeader(String authHeader) {
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            throw new CustomException("인증 헤더가 유효하지 않습니다.", HttpStatus.UNAUTHORIZED);
        }
        return authHeader.substring(7);
    }

    /**
     * 토큰의 만료 시간을 가져옴
     */
    public Date getExpirationFromToken(String token) {
        Claims claims = Jwts.parserBuilder()
            .setSigningKey(getSigningKey())
            .build()
            .parseClaimsJws(token)
            .getBody();
        return claims.getExpiration();
    }
}
