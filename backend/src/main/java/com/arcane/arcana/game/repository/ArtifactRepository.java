package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.Artifact;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface ArtifactRepository extends JpaRepository<Artifact, Long> {

    Optional<Artifact> findByUser_Id(Long userId);
}
