package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 맵 데이터 엔티티
 */
@Entity
@Table(name = "map_data")
@Getter
@Setter
@NoArgsConstructor
public class MapData {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String mapInfo;

    @Column(nullable = false)
    private Integer gameSession;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private User user;
}
