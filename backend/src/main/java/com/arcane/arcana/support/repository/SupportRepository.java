package com.arcane.arcana.support.repository;

import com.arcane.arcana.common.entity.SupportRequest;
import org.springframework.data.jpa.repository.JpaRepository;

public interface SupportRepository extends JpaRepository<SupportRequest, Long> {

}
