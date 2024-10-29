package com.arcane.arcana.subscribe.repository;

import com.arcane.arcana.common.entity.Subscribe;
import org.springframework.data.jpa.repository.JpaRepository;

public interface SubscribeRepository extends JpaRepository<Subscribe, Long> {

    boolean existsBySubscribe(String subscribe);
}
