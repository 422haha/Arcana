package com.arcane.arcana.user.controller;

import com.arcane.arcana.common.dto.ApiResponse;
import com.arcane.arcana.user.dto.TokenRefreshDto;
import com.arcane.arcana.user.service.UserService;
import com.arcane.arcana.common.util.JwtUtil;
import io.swagger.v3.oas.annotations.Operation;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

/**
 * 토큰 관련 요청을 처리하는 컨트롤러
 */
@RestController
@RequestMapping("/token")
public class TokenController {

    private final UserService userService;
    private final JwtUtil jwtUtil;

    public TokenController(UserService userService, JwtUtil jwtUtil) {
        this.userService = userService;
        this.jwtUtil = jwtUtil;
    }

    @Operation(
        summary = "토큰 갱신",
        description = "유효한 리프레시 토큰을 통해 새로운 액세스 토큰과 리프레시 토큰을 발급받습니다."
    )
    @PostMapping("/refresh")
    public ResponseEntity<ApiResponse<TokenRefreshDto>> refreshToken(
        @RequestBody TokenRefreshDto tokenRefreshDto) {
        String refreshToken = tokenRefreshDto.getRefreshToken();
        if (!jwtUtil.isTokenValid(refreshToken)) {
            return ResponseEntity.status(401).body(new ApiResponse<>("유효하지 않은 리프레시 토큰입니다.", null));
        }

        String email = jwtUtil.getEmailFromToken(refreshToken);
        String storedRefreshToken = userService.getStoredRefreshToken(email);

        if (!refreshToken.equals(storedRefreshToken)) {
            return ResponseEntity.status(401).body(new ApiResponse<>("일치하지 않는 리프레시 토큰입니다.", null));
        }

        String newAccessToken = jwtUtil.generateAccessToken(email);
        String newRefreshToken = jwtUtil.generateRefreshToken(email);

        userService.updateRefreshToken(email, newRefreshToken);

        TokenRefreshDto responseDto = new TokenRefreshDto();
        responseDto.setAccessToken(newAccessToken);
        responseDto.setRefreshToken(newRefreshToken);

        return ResponseEntity.ok(new ApiResponse<>("토큰 갱신 성공", responseDto));
    }
}
