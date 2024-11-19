package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import java.time.LocalDateTime;

/**
 * 문의 사항 엔티티
 */
@Entity
@Table(name = "SupportRequest")
@Getter
@Setter
@NoArgsConstructor
public class SupportRequest {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String category; // 문의 유형: 문제도움, 버그신고, 피드백제출

    @Column(nullable = false)
    private String email; // 사용자 이메일

    @Column(nullable = false)
    private String title; // 문의 제목

    @Column(length = 1000)
    private String description; // 문의 설명

    @Column
    private String screenshotPaths; // 스크린샷 파일 경로

    @Column(nullable = false)
    private LocalDateTime createdAt; // 문의 생성 날짜

    @PrePersist
    protected void onCreate() {
        createdAt = LocalDateTime.now();
    }
}
