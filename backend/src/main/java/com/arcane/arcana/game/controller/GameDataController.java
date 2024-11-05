package com.arcane.arcana.game.controller;

import com.arcane.arcana.common.dto.ApiResponse;
import com.arcane.arcana.common.exception.CustomException;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.game.dto.MapDataDto;
import com.arcane.arcana.game.dto.ProgressDataDto;
import com.arcane.arcana.game.service.MapDataService;
import com.arcane.arcana.game.service.ProgressDataService;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.entity.User;
import io.swagger.v3.oas.annotations.Operation;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@RequestMapping("/game-data")
public class GameDataController {

    private final MapDataService mapDataService;
    private final ProgressDataService progressDataService;
    private final JwtUtil jwtUtil;
    private final UserRepository userRepository;

    public GameDataController(MapDataService mapDataService,
        ProgressDataService progressDataService,
        JwtUtil jwtUtil, UserRepository userRepository) {
        this.mapDataService = mapDataService;
        this.progressDataService = progressDataService;
        this.jwtUtil = jwtUtil;
        this.userRepository = userRepository;
    }

    /*
    @Operation(summary = "맵 설정 저장", description = "사용자의 현재 게임 세션에 대한 맵 설정을 저장합니다.")
    @PostMapping(value = "/mapsetting", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public ResponseEntity<ApiResponse<String>> saveMapSetting(
        @RequestPart("mapSetting") MultipartFile mapSetting,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        mapDataService.saveMapSetting(userId, mapSetting);
        return ResponseEntity.ok(new ApiResponse<>("맵 정보 저장 성공", null));
    }
    */

    @Operation(summary = "맵 설정 저장", description = "사용자의 현재 게임 세션에 대한 맵 설정을 저장합니다.")
    @PostMapping("/mapsetting")
    public ResponseEntity<ApiResponse<String>> saveMapSetting(
        @RequestParam("mapSetting") String mapSetting,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        mapDataService.saveMapSetting(userId, mapSetting);
        return ResponseEntity.ok(new ApiResponse<>("맵 정보 저장 성공", null));
    }

    @Operation(summary = "맵 설정 가져오기", description = "사용자의 현재 게임 세션에 대한 맵 설정을 가져옵니다.")
    @GetMapping("/mapsetting")
    public ResponseEntity<ApiResponse<MapDataDto>> getMapSetting(HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        MapDataDto gameData = mapDataService.getMapSetting(userId);
        return ResponseEntity.ok(new ApiResponse<>("맵 정보 불러오기 성공", gameData));
    }

    /*
    @Operation(summary = "진행 정보 저장", description = "사용자의 현재 게임 세션에 대한 진행 정보를 저장합니다.")
    @PostMapping(value = "/progress", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public ResponseEntity<ApiResponse<String>> saveProgress(
        @RequestPart("progress") MultipartFile progress,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        progressDataService.saveProgress(userId, progress);
        return ResponseEntity.ok(new ApiResponse<>("진행 정보 저장 성공", null));
    }
    */

    @Operation(summary = "진행 정보 저장", description = "사용자의 현재 게임 세션에 대한 진행 정보를 저장합니다.")
    @PostMapping("/progress")
    public ResponseEntity<ApiResponse<String>> saveProgress(
        @RequestParam("progress") String progress,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        progressDataService.saveProgress(userId, progress);
        return ResponseEntity.ok(new ApiResponse<>("진행 정보 저장 성공", null));
    }

    @Operation(summary = "진행 정보 가져오기", description = "사용자의 현재 게임 세션에 대한 진행 정보를 가져옵니다.")
    @GetMapping("/progress")
    public ResponseEntity<ApiResponse<ProgressDataDto>> getProgress(HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        ProgressDataDto progressData = progressDataService.getProgress(userId);
        return ResponseEntity.ok(new ApiResponse<>("진행 정보 불러오기 성공", progressData));
    }

    @Operation(summary = "새 게임 세션 시작", description = "새로운 게임 세션을 시작합니다.")
    @PostMapping("/start")
    public ResponseEntity<ApiResponse<String>> startNewGameSession(HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        user.startNewGameSession();
        userRepository.save(user);

        return ResponseEntity.ok(new ApiResponse<>("새로운 게임 세션이 시작되었습니다.", null));
    }
}
