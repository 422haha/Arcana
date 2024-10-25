package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 카드 엔티티
 */
@Entity
@Table(name = "Card")
@Getter
@Setter
@NoArgsConstructor
public class Card {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user; // 외래 키: 사용자

    @Column(nullable = true)
    private String cardList; // 카드 덱 리스트
}
