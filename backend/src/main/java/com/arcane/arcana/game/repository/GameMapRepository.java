package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.GameMap;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface GameMapRepository extends JpaRepository<GameMap, Long> {

    Optional<GameMap> findByUser_Id(Long userId);
}
