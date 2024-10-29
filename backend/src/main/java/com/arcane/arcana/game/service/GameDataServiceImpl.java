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
import com.arcane.arcana.common.exception.CustomException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Optional;

@Service
public class GameDataServiceImpl implements GameDataService {

    private final UserRepository userRepository;
    private final GameDataRepository gameDataRepository;
    private final S3Service s3Service;
    private final ObjectMapper objectMapper;

    public GameDataServiceImpl(UserRepository userRepository,
        GameDataRepository gameDataRepository,
        S3Service s3Service) {
        this.userRepository = userRepository;
        this.gameDataRepository = gameDataRepository;
        this.s3Service = s3Service;
        this.objectMapper = new ObjectMapper();
    }

    @Override
    public void saveMapSetting(String email, MapSettingDto mapSettingDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "maps/" + user.getId() + ".json";
        try {
            s3Service.uploadFile(s3Path,
                mapSettingDto.getMapSetting().getBytes(StandardCharsets.UTF_8));
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        Optional<GameMap> existingGameMap = gameDataRepository.findGameMapByUserId(user.getId());
        GameMap gameMap;
        if (existingGameMap.isPresent()) {
            gameMap = existingGameMap.get();
            gameMap.setMapInfo(s3Path);
        } else {
            gameMap = new GameMap();
            gameMap.setUser(user);
            gameMap.setMapInfo(s3Path);
        }
        gameDataRepository.saveGameMap(gameMap);
    }

    @Override
    public void saveItem(String email, ItemDto itemDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "items/" + user.getId() + ".json";
        try {
            String itemData = objectMapper.writeValueAsString(itemDto.getItemInventory());
            s3Service.uploadFile(s3Path, itemData.getBytes(StandardCharsets.UTF_8));
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        Optional<Item> existingItem = gameDataRepository.findItemByUserId(user.getId());
        Item item;
        if (existingItem.isPresent()) {
            item = existingItem.get();
            item.setItemList(s3Path);
        } else {
            item = new Item();
            item.setUser(user);
            item.setItemList(s3Path);
        }
        gameDataRepository.saveItem(item);
    }

    @Override
    public void saveArtifact(String email, ArtifactDto artifactDto) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String s3Path = "artifacts/" + user.getId() + ".json";
        try {
            String artifactData = objectMapper.writeValueAsString(artifactDto.getArtifacts());
            s3Service.uploadFile(s3Path, artifactData.getBytes(StandardCharsets.UTF_8));
        } catch (Exception e) {
            throw new CustomException("S3 업로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        Optional<Artifact> existingArtifact = gameDataRepository.findArtifactByUserId(user.getId());
        Artifact artifact;
        if (existingArtifact.isPresent()) {
            artifact = existingArtifact.get();
            artifact.setArtifactList(s3Path);
        } else {
            artifact = new Artifact();
            artifact.setUser(user);
            artifact.setArtifactList(s3Path);
        }
        gameDataRepository.saveArtifact(artifact);
    }

    @Override
    public ProgressDto getProgress(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        GameMap gameMap = gameDataRepository.findGameMapByUserId(user.getId())
            .orElseThrow(() -> new CustomException("진행 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String mapData;
        try {
            mapData = new String(s3Service.downloadFile(gameMap.getMapInfo()),
                StandardCharsets.UTF_8);
        } catch (Exception e) {
            throw new CustomException("S3 다운로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        return new ProgressDto(user.getId().toString(), mapData);
    }

    @Override
    public ItemDto getItem(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Item item = gameDataRepository.findItemByUserId(user.getId())
            .orElseThrow(() -> new CustomException("아이템 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String itemData;
        try {
            itemData = new String(s3Service.downloadFile(item.getItemList()),
                StandardCharsets.UTF_8);
        } catch (Exception e) {
            throw new CustomException("S3 다운로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        List<String> itemInventory;
        try {
            itemInventory = objectMapper.readValue(itemData, new TypeReference<List<String>>() {
            });
        } catch (Exception e) {
            throw new CustomException("아이템 데이터 파싱 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        return new ItemDto(user.getId().toString(), itemInventory);
    }

    @Override
    public ArtifactDto getArtifact(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Artifact artifact = gameDataRepository.findArtifactByUserId(user.getId())
            .orElseThrow(() -> new CustomException("아티팩트 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String artifactData;
        try {
            artifactData = new String(s3Service.downloadFile(artifact.getArtifactList()),
                StandardCharsets.UTF_8);
        } catch (Exception e) {
            throw new CustomException("S3 다운로드 중 오류가 발생했습니다.", HttpStatus.INTERNAL_SERVER_ERROR);
        }

        List<String> artifactList;
        try {
            artifactList = objectMapper.readValue(artifactData, new TypeReference<List<String>>() {
            });
        } catch (Exception e) {
            throw new CustomException("아티팩트 데이터 파싱 중 오류가 발생했습니다.",
                HttpStatus.INTERNAL_SERVER_ERROR);
        }

        return new ArtifactDto(user.getId().toString(), artifactList);
    }
}
