package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 구독 엔티티
 */
@Entity
@Table(name = "Subscribe")
@Getter
@Setter
@NoArgsConstructor
public class Subscribe {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private String subscribe; // 구독자의 이메일
}
