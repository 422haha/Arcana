package com.arcane.arcana.game.controller;

import com.arcane.arcana.game.dto.MapSettingDto;
import com.arcane.arcana.game.dto.ItemDto;
import com.arcane.arcana.game.dto.ArtifactDto;
import com.arcane.arcana.game.dto.ProgressDto;
import com.arcane.arcana.game.service.GameDataService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.exception.CustomException;
import com.arcane.arcana.common.dto.ApiResponse;
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
    public ResponseEntity<ApiResponse> saveMapSetting(@RequestBody MapSettingDto mapSettingDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        gameDataService.saveMapSetting(email, mapSettingDto);
        return ResponseEntity.ok(new ApiResponse("플레이 정보 저장 성공", null));
    }

    @PutMapping("/item")
    public ResponseEntity<ApiResponse> saveItem(@RequestBody ItemDto itemDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        gameDataService.saveItem(email, itemDto);
        return ResponseEntity.ok(new ApiResponse("아이템 정보 저장 성공", null));
    }

    @PutMapping("/artifact")
    public ResponseEntity<ApiResponse> saveArtifact(@RequestBody ArtifactDto artifactDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        gameDataService.saveArtifact(email, artifactDto);
        return ResponseEntity.ok(new ApiResponse("아티팩트 정보 저장 성공", null));
    }

    @GetMapping("/progress")
    public ResponseEntity<ApiResponse> getProgress(HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        ProgressDto progress = gameDataService.getProgress(email);
        return ResponseEntity.ok(new ApiResponse("플레이 정보 불러오기 성공", progress));
    }

    @GetMapping("/item")
    public ResponseEntity<ApiResponse> getItem(HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        ItemDto items = gameDataService.getItem(email);
        return ResponseEntity.ok(new ApiResponse("아이템 정보 불러오기 성공", items));
    }

    @GetMapping("/artifact")
    public ResponseEntity<ApiResponse> getArtifact(HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        ArtifactDto artifacts = gameDataService.getArtifact(email);
        return ResponseEntity.ok(new ApiResponse("아티팩트 정보 불러오기 성공", artifacts));
    }
}
