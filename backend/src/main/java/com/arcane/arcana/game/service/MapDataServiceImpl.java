package com.arcane.arcana.game.service;

import com.arcane.arcana.common.entity.MapData;
import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.game.dto.MapDataDto;
import com.arcane.arcana.game.repository.MapDataRepository;
import com.arcane.arcana.common.exception.CustomException;
import com.arcane.arcana.common.service.PresignedUrlService;
import com.arcane.arcana.common.service.S3Service;
import com.arcane.arcana.user.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;

@Service
public class MapDataServiceImpl implements MapDataService {

    private final MapDataRepository mapDataRepository;
    private final UserRepository userRepository;

    /*
    private final S3Service s3Service;
    private final PresignedUrlService presignedUrlService;
    */

    public MapDataServiceImpl(MapDataRepository mapDataRepository, UserRepository userRepository) {
        this.mapDataRepository = mapDataRepository;
        this.userRepository = userRepository;
    }

    /*
    @Override
    @Transactional
    public void saveMapSetting(Long userId, MultipartFile mapSetting) {
        String key = "maps/" + userId;
        try {
            String mapPath = s3Service.uploadFile(key, mapSetting.getBytes());

            User user = userRepository.findById(userId)
                .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

            Integer currentGameSession = user.getGameSession();

            mapDataRepository.findByUserAndGameSession(user, currentGameSession)
                .ifPresentOrElse(mapData -> {
                    mapData.setMapInfo(mapPath);
                    mapDataRepository.save(mapData);
                }, () -> {
                    MapData newData = new MapData();
                    newData.setUser(user);
                    newData.setGameSession(currentGameSession);
                    newData.setMapInfo(mapPath);
                    mapDataRepository.save(newData);
                });
        } catch (IOException e) {
            throw new CustomException("맵 설정 파일 업로드 실패", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
    */

    @Override
    @Transactional
    public void saveMapSetting(Long userId, String mapSetting) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        mapDataRepository.findByUserAndGameSession(user, currentGameSession)
            .ifPresentOrElse(mapData -> {
                mapData.setMapInfo(mapSetting);
                mapDataRepository.save(mapData);
            }, () -> {
                MapData newData = new MapData();
                newData.setUser(user);
                newData.setGameSession(currentGameSession);
                newData.setMapInfo(mapSetting);
                mapDataRepository.save(newData);
            });
    }

    @Override
    public MapDataDto getMapSetting(Long userId) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        MapData mapData = mapDataRepository.findByUserAndGameSession(user, currentGameSession)
            .orElseThrow(() -> new CustomException("맵 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        /*
        String presignedUrl = presignedUrlService.getPresignedUrl(mapData.getMapInfo());
        return new MapDataDto(presignedUrl);
        */

        return new MapDataDto(mapData.getMapInfo());
    }
}
