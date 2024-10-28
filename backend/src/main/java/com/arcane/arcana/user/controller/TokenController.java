package com.arcane.arcana.user.controller;

import com.arcane.arcana.user.dto.TokenRefreshDto;
import com.arcane.arcana.user.service.UserService;
import com.arcane.arcana.common.util.JwtUtil;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/token")
public class TokenController {

    private final UserService userService;
    private final JwtUtil jwtUtil;

    public TokenController(UserService userService, JwtUtil jwtUtil) {
        this.userService = userService;
        this.jwtUtil = jwtUtil;
    }

    @PostMapping("/refresh")
    public ResponseEntity<?> refreshToken(@RequestBody TokenRefreshDto tokenRefreshDto) {
        String refreshToken = tokenRefreshDto.getRefreshToken();
        if (!jwtUtil.isTokenValid(refreshToken)) {
            return ResponseEntity.status(401).body("{\"message\": \"토큰 갱신 실패\", \"data\": null}");
        }

        String email = jwtUtil.getUsernameFromToken(refreshToken);
        String newAccessToken = jwtUtil.generateAccessToken(email);
        String newRefreshToken = jwtUtil.generateRefreshToken(email);

        userService.updateRefreshToken(email, newRefreshToken);

        return ResponseEntity.ok("{\"message\": \"토큰 갱신 성공\", \"data\": {\"accessToken\": \""
            + newAccessToken + "\", \"refreshToken\": \"" + newRefreshToken + "\"}}");
    }
}
