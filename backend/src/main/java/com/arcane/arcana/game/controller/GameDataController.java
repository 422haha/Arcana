package com.arcane.arcana.game.controller;

import com.arcane.arcana.game.dto.MapSettingDto;
import com.arcane.arcana.game.dto.ItemDto;
import com.arcane.arcana.game.dto.ArtifactDto;
import com.arcane.arcana.game.dto.ProgressDto;
import com.arcane.arcana.game.service.GameDataService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import jakarta.servlet.http.HttpServletRequest;

@RestController
@RequestMapping("/game-data")
public class GameDataController {

    private final GameDataService gameDataService;
    private final JwtUtil jwtUtil;

    public GameDataController(GameDataService gameDataService, JwtUtil jwtUtil) {
        this.gameDataService = gameDataService;
        this.jwtUtil = jwtUtil;
    }

    @PutMapping("/mapsetting")
    public ResponseEntity<?> saveMapSetting(@RequestBody MapSettingDto mapSettingDto,
        HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        gameDataService.saveMapSetting(email, mapSettingDto);
        return ResponseEntity.ok(new ApiResponse("플레이 정보 저장 성공", null));
    }

    @PutMapping("/item")
    public ResponseEntity<?> saveItem(@RequestBody ItemDto itemDto, HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        gameDataService.saveItem(email, itemDto);
        return ResponseEntity.ok(new ApiResponse("아이템 정보 저장 성공", null));
    }

    @PutMapping("/artifact")
    public ResponseEntity<?> saveArtifact(@RequestBody ArtifactDto artifactDto,
        HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        gameDataService.saveArtifact(email, artifactDto);
        return ResponseEntity.ok(new ApiResponse("아티팩트 정보 저장 성공", null));
    }

    @GetMapping("/progress")
    public ResponseEntity<?> getProgress(HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        ProgressDto progress = gameDataService.getProgress(email);
        return ResponseEntity.ok(new ApiResponse("플레이 정보 불러오기 성공", progress));
    }

    @GetMapping("/item")
    public ResponseEntity<?> getItem(HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        ItemDto items = gameDataService.getItem(email);
        return ResponseEntity.ok(new ApiResponse("아이템 정보 불러오기 성공", items));
    }

    @GetMapping("/artifact")
    public ResponseEntity<?> getArtifact(HttpServletRequest request) {
        String email = extractEmailFromRequest(request);
        ArtifactDto artifacts = gameDataService.getArtifact(email);
        return ResponseEntity.ok(new ApiResponse("아티팩트 정보 불러오기 성공", artifacts));
    }

    private String extractEmailFromRequest(HttpServletRequest request) {
        String authHeader = request.getHeader("Authorization");
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            throw new CustomException("인증 헤더가 유효하지 않습니다.", HttpStatus.UNAUTHORIZED);
        }
        String token = authHeader.substring(7);
        return jwtUtil.getUsernameFromToken(token);
    }

    // 공통 응답 포맷을 위한 클래스 추가
    static class ApiResponse {

        private String message;
        private Object data;

        public ApiResponse(String message, Object data) {
            this.message = message;
            this.data = data;
        }

        // Getter 및 Setter
        public String getMessage() {
            return message;
        }

        public void setMessage(String message) {
            this.message = message;
        }

        public Object getData() {
            return data;
        }

        public void setData(Object data) {
            this.data = data;
        }
    }
}
