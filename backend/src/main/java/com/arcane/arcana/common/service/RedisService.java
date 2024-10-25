package com.arcane.arcana.common.service;

import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Service;

import java.time.Duration;

/**
 * Redis 관련 기능을 제공
 */
@Service
public class RedisService {

    private final RedisTemplate<String, Object> redisTemplate;

    public RedisService(RedisTemplate<String, Object> redisTemplate) {
        this.redisTemplate = redisTemplate;
    }

    /**
     * 값을 설정
     */
    public void setValue(String key, Object value) {
        redisTemplate.opsForValue().set(key, value);
    }

    /**
     * TTL과 함께 값을 설정
     */
    public void setValueWithTTL(String key, Object value, long timeoutInMinutes) {
        redisTemplate.opsForValue().set(key, value, Duration.ofMinutes(timeoutInMinutes));
    }

    /**
     * 값을 가져옴
     */
    public Object getValue(String key) {
        return redisTemplate.opsForValue().get(key);
    }

    /**
     * 값을 삭제
     */
    public void deleteValue(String key) {
        redisTemplate.delete(key);
    }

    /**
     * 키의 TTL을 가져옴
     */
    public Long getTTL(String key) {
        return redisTemplate.getExpire(key);
    }

    /**
     * 문자열 값을 설정하는 메서드
     */
    public void setStringValue(String key, String value, long timeoutInMinutes) {
        redisTemplate.opsForValue().set(key, value, Duration.ofMinutes(timeoutInMinutes));
    }

    /**
     * 문자열 값을 가져오는 메서드
     */
    public String getStringValue(String key) {
        Object value = redisTemplate.opsForValue().get(key);
        return value != null ? value.toString() : null;
    }
}
