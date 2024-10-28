package com.arcane.arcana.game.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.common.entity.GameMap;
import com.arcane.arcana.common.entity.Item;
import com.arcane.arcana.common.entity.Artifact;
import com.arcane.arcana.game.dto.MapSettingDto;
import com.arcane.arcana.game.dto.ItemDto;
import com.arcane.arcana.game.dto.ArtifactDto;
import com.arcane.arcana.game.dto.ProgressDto;
import com.arcane.arcana.game.repository.GameDataRepository;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.service.S3Service;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class GameDataServiceImpl implements GameDataService {

    private final UserRepository userRepository;
    private final GameDataRepository gameDataRepository;
    private final S3Service s3Service;
    private final RedisService redisService;

    @Autowired
    public GameDataServiceImpl(UserRepository userRepository,
        GameDataRepository gameDataRepository,
        S3Service s3Service,
        RedisService redisService) {
        this.userRepository = userRepository;
        this.gameDataRepository = gameDataRepository;
        this.s3Service = s3Service;
        this.redisService = redisService;
    }

    @Override
    public void saveMapSetting(String email, MapSettingDto mapSettingDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "maps/" + user.getId() + ".json";
        try {
            s3Service.uploadFile(s3Path, mapSettingDto.getMapSetting().getBytes());
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        GameMap gameMap = new GameMap();
        gameMap.setUser(user);
        gameMap.setMapInfo(s3Path);
        gameDataRepository.saveGameMap(gameMap);
    }

    @Override
    public void saveItem(String email, ItemDto itemDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "items/" + user.getId() + ".json";
        try {
            s3Service.uploadFile(s3Path, itemDto.getItemInventory().toString().getBytes());
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        Item item = new Item();
        item.setUser(user);
        item.setItemList(s3Path);
        gameDataRepository.saveItem(item);
    }

    @Override
    public void saveArtifact(String email, ArtifactDto artifactDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "artifacts/" + user.getId() + ".json";
        try {
            s3Service.uploadFile(s3Path, artifactDto.getArtifacts().toString().getBytes());
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        Artifact artifact = new Artifact();
        artifact.setUser(user);
        artifact.setArtifactList(s3Path);
        gameDataRepository.saveArtifact(artifact);
    }

    @Override
    public ProgressDto getProgress(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        GameMap gameMap = gameDataRepository.findGameMapByUserId(user.getId())
            .orElseThrow(() -> new CustomException("진행 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        return new ProgressDto(user.getId().toString(), gameMap.getMapInfo());
    }

    @Override
    public ItemDto getItem(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        List<Item> items = gameDataRepository.findItemsByUserId(user.getId());
        List<String> itemList = items.stream()
            .map(Item::getItemList)
            .collect(Collectors.toList());

        return new ItemDto(user.getId().toString(), itemList);
    }

    @Override
    public ArtifactDto getArtifact(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        List<Artifact> artifacts = gameDataRepository.findArtifactsByUserId(user.getId());
        List<String> artifactList = artifacts.stream()
            .map(Artifact::getArtifactList)
            .collect(Collectors.toList());

        return new ArtifactDto(user.getId().toString(), artifactList);
    }
}
