package com.arcane.arcana.common.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

/**
 * 서버 상태를 확인
 */
@RestController
public class HealthCheckController {

    /**
     * 헬스 체크 엔드포인트
     *
     * @return 서버 상태 메시지 반환
     */
    @GetMapping("/health")
    public ResponseEntity<String> healthCheck() {
        return ResponseEntity.ok("OK");
    }
}
