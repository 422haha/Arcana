package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.Card;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface CardRepository extends JpaRepository<Card, Long> {

    Optional<Card> findByUser_Id(Long userId);
}
