package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.Card;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface CardRepository extends JpaRepository<Card, Long> {

    List<Card> findByUser_Id(Long userId);
}
