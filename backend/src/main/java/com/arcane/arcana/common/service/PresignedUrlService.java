package com.arcane.arcana.common.service;

import java.time.Duration;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Service;

/**
 * 프리사인드 URL을 관리
 */
@Service
public class PresignedUrlService {

    private final S3Service s3Service;
    private final RedisTemplate<String, Object> redisTemplate;

    @Value("${presigned.url.cache.duration.minutes:120}")  // 기본값 120분
    private long cacheDuration;

    @Autowired
    public PresignedUrlService(S3Service s3Service, RedisTemplate<String, Object> redisTemplate) {
        this.s3Service = s3Service;
        this.redisTemplate = redisTemplate;
    }

    /**
     * 프리사인드 URL을 가져옴
     *
     * @param objectKey S3 객체 키
     * @return 프리사인드 URL 반환
     */
    public String getPresignedUrl(String objectKey) {
        String cacheKey = "presigned_url:" + objectKey;

        // Redis에서 캐시된 URL 가져오기
        String cachedUrl = (String) redisTemplate.opsForValue().get(cacheKey);
        if (cachedUrl != null) {
            return cachedUrl;
        }

        // S3에서 프리사인드 URL 생성
        String presignedUrl = s3Service.generatePresignedUrl(objectKey, cacheDuration).toString();

        if (presignedUrl != null) {
            // Redis에 캐시 저장
            redisTemplate.opsForValue()
                .set(cacheKey, presignedUrl, Duration.ofMinutes(cacheDuration));
        }

        return presignedUrl;
    }
}
