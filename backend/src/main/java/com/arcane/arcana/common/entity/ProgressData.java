package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 진행 데이터 엔티티
 */
@Entity
@Table(name = "progress_data")
@Getter
@Setter
@NoArgsConstructor
public class ProgressData {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String progressInfo;

    @Column(nullable = false)
    private Integer gameSession;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;
}
