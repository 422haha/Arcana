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
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

/**
 * 게임 데이터 관련 요청을 처리하는 컨트롤러
 */
@RestController
@RequestMapping("/game-data")
public class GameDataController {

    private final MapDataService mapDataService;
    private final ProgressDataService progressDataService;
    private final JwtUtil jwtUtil;
    private final UserRepository userRepository;

    public GameDataController(MapDataService mapDataService,
        ProgressDataService progressDataService, JwtUtil jwtUtil,
        UserRepository userRepository) {
        this.mapDataService = mapDataService;
        this.progressDataService = progressDataService;
        this.jwtUtil = jwtUtil;
        this.userRepository = userRepository;
    }

    @PutMapping(value = "/mapsetting", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public ResponseEntity<ApiResponse<String>> saveMapSetting(
        @RequestPart("mapSetting") MultipartFile mapSetting,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        mapDataService.saveMapSetting(userId, mapSetting);
        return ResponseEntity.ok(new ApiResponse<>("맵 정보 저장 성공", null));
    }

    @GetMapping("/mapsetting")
    public ResponseEntity<ApiResponse<MapDataDto>> getMapSetting(HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        MapDataDto gameData = mapDataService.getMapSetting(userId);
        return ResponseEntity.ok(new ApiResponse<>("맵 정보 불러오기 성공", gameData));
    }

    @PutMapping(value = "/progress", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public ResponseEntity<ApiResponse<String>> saveProgress(
        @RequestPart("progress") MultipartFile progress,
        HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        progressDataService.saveProgress(userId, progress);
        return ResponseEntity.ok(new ApiResponse<>("진행 정보 저장 성공", null));
    }

    @GetMapping("/progress")
    public ResponseEntity<ApiResponse<ProgressDataDto>> getProgress(HttpServletRequest request) {
        Long userId = jwtUtil.getUserIdFromToken(request.getHeader("Authorization"));
        ProgressDataDto progressData = progressDataService.getProgress(userId);
        return ResponseEntity.ok(new ApiResponse<>("진행 정보 불러오기 성공", progressData));
    }

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
