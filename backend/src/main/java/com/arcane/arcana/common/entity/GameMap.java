package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 맵 엔티티
 */
@Entity
@Table(name = "GameMap")
@Getter
@Setter
@NoArgsConstructor
public class GameMap {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user; // 외래 키: 사용자

    @Column(nullable = false)
    private String mapInfo; // S3 맵 정보 JSON 링크
}
