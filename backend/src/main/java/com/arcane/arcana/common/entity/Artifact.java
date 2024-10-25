package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 아티팩트 엔티티
 */
@Entity
@Table(name = "Artifact")
@Getter
@Setter
@NoArgsConstructor
public class Artifact {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user; // 외래 키: 사용자

    @Column(nullable = true)
    private String artifactList; // 아티팩트 리스트
}
