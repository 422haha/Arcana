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
import org.springframework.web.multipart.MultipartFile;

import java.io.IOException;

@Service
public class ProgressDataServiceImpl implements ProgressDataService {

    private final ProgressDataRepository progressDataRepository;
    private final S3Service s3Service;
    private final PresignedUrlService presignedUrlService;
    private final UserRepository userRepository;

    public ProgressDataServiceImpl(ProgressDataRepository progressDataRepository,
        S3Service s3Service,
        PresignedUrlService presignedUrlService, UserRepository userRepository) {
        this.progressDataRepository = progressDataRepository;
        this.s3Service = s3Service;
        this.presignedUrlService = presignedUrlService;
        this.userRepository = userRepository;
    }

    @Override
    @Transactional
    public void saveProgress(Long userId, MultipartFile progress) {
        String key = "progress/" + userId;
        try {
            String progressPath = s3Service.uploadFile(key, progress.getBytes());

            User user = userRepository.findById(userId)
                .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

            Integer currentGameSession = user.getGameSession();

            progressDataRepository.findByUserAndGameSession(user, currentGameSession)
                .ifPresentOrElse(progressData -> {
                    progressData.setProgressInfo(progressPath);
                    progressDataRepository.save(progressData);
                }, () -> {
                    ProgressData newData = new ProgressData();
                    newData.setUser(user);
                    newData.setGameSession(currentGameSession);
                    newData.setProgressInfo(progressPath);
                    progressDataRepository.save(newData);
                });
        } catch (IOException e) {
            throw new CustomException("진행 정보 파일 업로드 실패", HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    @Override
    public ProgressDataDto getProgress(Long userId) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        Integer currentGameSession = user.getGameSession();

        ProgressData progressData = progressDataRepository.findByUserAndGameSession(user,
                currentGameSession)
            .orElseThrow(() -> new CustomException("진행 정보를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String presignedUrl = presignedUrlService.getPresignedUrl(progressData.getProgressInfo());
        return new ProgressDataDto(presignedUrl);
    }
}
