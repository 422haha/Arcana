package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.ProgressData;
import com.arcane.arcana.common.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface ProgressDataRepository extends JpaRepository<ProgressData, Long> {

    Optional<ProgressData> findByUserAndGameSession(User user, Integer gameSession);
}
