package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.CreationTimestamp;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.time.LocalDateTime;
import java.util.List;

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
    private String language = "ko"; // 사용자 언어 설정, 기본값 "ko"

    @Column(nullable = false)
    private Integer gameSession = 0;

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<MapData> mapDataList;

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<ProgressData> progressDataList;

    @CreationTimestamp
    @Column(updatable = false)
    private LocalDateTime createdAt; // 회원가입 날짜

    @Column(nullable = false)
    private boolean isDeleted = false; // 탈퇴 여부

    @Column
    private LocalDateTime deletedAt; // 탈퇴 시각

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

    /**
     * 게임 시작 시 gameSession 증가
     */
    public void startNewGameSession() {
        this.gameSession += 1;
    }

    /**
     * 회원 탈퇴 처리
     */
    public void deactivate() {
        this.isDeleted = true;
        this.deletedAt = LocalDateTime.now();
    }

    /**
     * 회원 복구 처리
     */
    public void reactivate() {
        this.isDeleted = false;
        this.deletedAt = null;
    }
}
