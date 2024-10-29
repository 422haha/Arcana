package com.arcane.arcana.game.repository;

import com.arcane.arcana.common.entity.Artifact;
import com.arcane.arcana.common.entity.Card;
import com.arcane.arcana.common.entity.Item;
import com.arcane.arcana.common.entity.GameMap;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

import java.util.Optional;

@Repository
public class GameDataRepository {

    private final CardRepository cardRepository;
    private final ArtifactRepository artifactRepository;
    private final ItemRepository itemRepository;
    private final GameMapRepository gameMapRepository;

    public GameDataRepository(CardRepository cardRepository,
        ArtifactRepository artifactRepository,
        ItemRepository itemRepository,
        GameMapRepository gameMapRepository) {
        this.cardRepository = cardRepository;
        this.artifactRepository = artifactRepository;
        this.itemRepository = itemRepository;
        this.gameMapRepository = gameMapRepository;
    }

    @Transactional
    public void saveCard(Card card) {
        cardRepository.save(card);
    }

    @Transactional
    public void saveArtifact(Artifact artifact) {
        artifactRepository.save(artifact);
    }

    @Transactional
    public void saveItem(Item item) {
        itemRepository.save(item);
    }

    @Transactional
    public void saveGameMap(GameMap gameMap) {
        gameMapRepository.save(gameMap);
    }

    @Transactional(readOnly = true)
    public Optional<Card> findCardByUserId(Long userId) {
        return cardRepository.findByUser_Id(userId);
    }

    @Transactional(readOnly = true)
    public Optional<Artifact> findArtifactByUserId(Long userId) {
        return artifactRepository.findByUser_Id(userId);
    }

    @Transactional(readOnly = true)
    public Optional<Item> findItemByUserId(Long userId) {
        return itemRepository.findByUser_Id(userId);
    }

    @Transactional(readOnly = true)
    public Optional<GameMap> findGameMapByUserId(Long userId) {
        return gameMapRepository.findByUser_Id(userId);
    }
}
