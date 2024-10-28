package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.Item;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.List;

public interface ItemRepository extends JpaRepository<Item, Long> {

    List<Item> findByUser_Id(Long userId);
}
