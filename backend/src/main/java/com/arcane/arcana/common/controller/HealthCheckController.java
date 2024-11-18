package com.arcane.arcana.common.controller;

import io.swagger.v3.oas.annotations.Operation;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

/**
 * 서버 상태를 확인하는 헬스 체크 컨트롤러
 */
@RestController
public class HealthCheckController {

    @Operation(summary = "서버 헬스 체크", description = "서버의 상태를 확인하여 'OK' 메시지를 반환합니다.")
    @GetMapping("/health")
    public ResponseEntity<String> healthCheck() {
        return ResponseEntity.ok("OK");
    }
}
