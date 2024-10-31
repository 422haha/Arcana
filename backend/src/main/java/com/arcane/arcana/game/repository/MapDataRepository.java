package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.MapData;
import com.arcane.arcana.common.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface MapDataRepository extends JpaRepository<MapData, Long> {

    Optional<MapData> findByUserAndGameSession(User user, Integer gameSession);
}
