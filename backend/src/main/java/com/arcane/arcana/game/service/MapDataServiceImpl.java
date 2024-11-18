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

/**
 * 맵 데이터 관련 서비스 구현 클래스
 */
@Service
public class MapDataServiceImpl implements MapDataService {

    private final MapDataRepository mapDataRepository;
    private final UserRepository userRepository;
    private final S3Service s3Service;
    private final PresignedUrlService presignedUrlService;

    public MapDataServiceImpl(MapDataRepository mapDataRepository, UserRepository userRepository,
        S3Service s3Service, PresignedUrlService presignedUrlService) {
        this.mapDataRepository = mapDataRepository;
        this.userRepository = userRepository;
        this.s3Service = s3Service;
        this.presignedUrlService = presignedUrlService;
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
        // 사용자 조회
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        // JSON 파일 생성 (맵 설정 문자열을 JSON 형식으로 변환)
        String jsonContent = "{\"mapSetting\": \"" + escapeJson(mapSetting) + "\"}";

        // S3에 업로드할 키 생성 (예: maps/{userId}/{gameSession}.json)
        String key = String.format("maps/%d/%d.json", userId, currentGameSession);

        // S3에 파일 업로드
        s3Service.uploadFile(key, jsonContent.getBytes());

        // 데이터베이스에 S3 키 저장
        mapDataRepository.findByUserAndGameSession(user, currentGameSession)
            .ifPresentOrElse(mapData -> {
                mapData.setMapInfo(key);
                mapDataRepository.save(mapData);
            }, () -> {
                MapData newData = new MapData();
                newData.setUser(user);
                newData.setGameSession(currentGameSession);
                newData.setMapInfo(key);
                mapDataRepository.save(newData);
            });
    }

    @Override
    public MapDataDto getMapSetting(Long userId) {
        // 사용자 조회
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        // 맵 데이터 조회
        MapData mapData = mapDataRepository.findByUserAndGameSession(user, currentGameSession)
            .orElseThrow(() -> new CustomException("맵 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        // 프리사인드 URL 생성
        String presignedUrl = presignedUrlService.getPresignedUrl(mapData.getMapInfo());

        return new MapDataDto(presignedUrl);
    }

    /**
     * JSON 내의 특수 문자를 이스케이프 처리
     *
     * @param text 원본 문자열
     * @return 이스케이프된 문자열
     */
    private String escapeJson(String text) {
        return text.replace("\"", "\\\"")
            .replace("\n", "\\n")
            .replace("\r", "\\r")
            .replace("\t", "\\t");
    }
}
