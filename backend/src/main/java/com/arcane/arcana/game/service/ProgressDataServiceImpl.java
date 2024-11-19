package com.arcane.arcana.game.service;

import com.arcane.arcana.common.entity.ProgressData;
import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.game.dto.ProgressDataDto;
import com.arcane.arcana.game.repository.ProgressDataRepository;
import com.arcane.arcana.common.exception.CustomException;
import com.arcane.arcana.common.service.PresignedUrlService;
import com.arcane.arcana.common.service.S3Service;
import com.arcane.arcana.user.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

/**
 * 진행 데이터 관련 서비스 구현 클래스
 */
@Service
public class ProgressDataServiceImpl implements ProgressDataService {

    private final ProgressDataRepository progressDataRepository;
    private final UserRepository userRepository;
    private final S3Service s3Service;
    private final PresignedUrlService presignedUrlService;

    public ProgressDataServiceImpl(ProgressDataRepository progressDataRepository,
        UserRepository userRepository,
        S3Service s3Service,
        PresignedUrlService presignedUrlService) {
        this.progressDataRepository = progressDataRepository;
        this.userRepository = userRepository;
        this.s3Service = s3Service;
        this.presignedUrlService = presignedUrlService;
    }

    @Override
    @Transactional
    public void saveProgress(Long userId, String progress) {
        // 사용자 조회
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        // JSON 파일 생성 (진행 정보 문자열을 JSON 형식으로 변환)
        String jsonContent = "{\"progress\": \"" + escapeJson(progress) + "\"}";

        // S3에 업로드할 키 생성 (예: progress/{userId}/{gameSession}.json)
        String key = String.format("progress/%d/%d.json", userId, currentGameSession);

        // S3에 파일 업로드
        s3Service.uploadFile(key, jsonContent.getBytes());

        // 데이터베이스에 S3 키 저장
        progressDataRepository.findByUserAndGameSession(user, currentGameSession)
            .ifPresentOrElse(progressData -> {
                progressData.setProgressInfo(key);
                progressDataRepository.save(progressData);
            }, () -> {
                ProgressData newData = new ProgressData();
                newData.setUser(user);
                newData.setGameSession(currentGameSession);
                newData.setProgressInfo(key);
                progressDataRepository.save(newData);
            });

        // 진행 정보 저장 시 프리사인드 URL을 반환하지 않음
    }

    @Override
    public ProgressDataDto getProgress(Long userId) {
        // 사용자 조회
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        // 진행 데이터 조회
        ProgressData progressData = progressDataRepository.findByUserAndGameSession(user,
                currentGameSession)
            .orElseThrow(() -> new CustomException("진행 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        // 프리사인드 URL 생성
        String presignedUrl = presignedUrlService.getPresignedUrl(progressData.getProgressInfo());

        return new ProgressDataDto(presignedUrl);
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
