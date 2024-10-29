package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.*;
import org.springframework.security.crypto.password.PasswordEncoder;

/**
 * 사용자 엔티티
 */
@Entity
@Table(name = "users")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, unique = true)
    private String email; // 사용자 이메일

    @Column(nullable = false, unique = true)
    private String nickname; // 사용자 닉네임

    @Column(nullable = false)
    private String password; // 사용자 비밀번호

    @Column(nullable = false)
    private boolean isDeleted = false; // 탈퇴 여부

    @Column(nullable = false)
    private boolean isEmailVerified = false; // 이메일 인증 여부

    @Column(nullable = false)
    private String language = "ko"; // 사용자 언어 설정, 기본값 "ko"

    @Column(nullable = false)
    private Integer money = 0; // 초기 값 0

    @Column(nullable = false)
    private Integer health = 100; // 초기 값 100

    /**
     * 비밀번호를 암호화
     */
    public void encodePassword(String rawPassword, PasswordEncoder passwordEncoder) {
        this.password = passwordEncoder.encode(rawPassword);
    }

    /**
     * 비밀번호 검증
     */
    public boolean isPasswordMatch(String rawPassword, PasswordEncoder passwordEncoder) {
        return passwordEncoder.matches(rawPassword, this.password);
    }
}
