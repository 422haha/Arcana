package com.arcane.arcana.common.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 아이템 엔티티
 */
@Entity
@Table(name = "Item")
@Getter
@Setter
@NoArgsConstructor
public class Item {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "user_id", nullable = false)
    private User user; // 외래 키: 사용자

    @Column(nullable = true)
    private String itemList; // 아이템 리스트
}
