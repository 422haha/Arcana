package com.arcane.arcana.user.controller;

import com.arcane.arcana.user.dto.LoginDto;
import com.arcane.arcana.user.dto.RegisterDto;
import com.arcane.arcana.user.dto.UpdateDto;
import com.arcane.arcana.user.service.UserService;
import com.arcane.arcana.common.util.JwtUtil;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

/**
 * 사용자 관련 요청을 처리하는 컨트롤러
 */
@RestController
@RequestMapping("/user")
public class UserController {

    private final UserService userService;
    private final JwtUtil jwtUtil;

    public UserController(UserService userService, JwtUtil jwtUtil) {
        this.userService = userService;
        this.jwtUtil = jwtUtil;
    }

    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody RegisterDto registerDto) {
        userService.registerUser(registerDto);
        return ResponseEntity.ok("회원 가입이 완료되었습니다. 이메일 인증을 진행해주세요.");
    }

    @GetMapping("/verify-email")
    public ResponseEntity<String> verifyEmail(@RequestParam String email,
        @RequestParam String token) {
        userService.verifyEmail(email, token);
        return ResponseEntity.ok("이메일 인증이 완료되었습니다.");
    }

    @PostMapping("/login")
    public ResponseEntity<String> login(@RequestBody LoginDto loginDto) {
        String accessToken = userService.login(loginDto.getEmail(), loginDto.getPassword());
        return ResponseEntity.ok("로그인 성공. 액세스 토큰: " + accessToken);
    }

    @PutMapping("/update")
    public ResponseEntity<String> updateUser(@RequestBody UpdateDto updateDto,
        HttpServletRequest request) {
        String authHeader = request.getHeader("Authorization");
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            return ResponseEntity.status(401).body("인증 헤더가 유효하지 않습니다.");
        }
        String token = authHeader.substring(7);
        String email = jwtUtil.getUsernameFromToken(token);
        Long userId = userService.getUserIdByEmail(email);

        userService.updateUser(userId, updateDto);
        return ResponseEntity.ok("사용자 정보가 수정되었습니다.");
    }

    @PostMapping("/logout")
    public ResponseEntity<String> logout(HttpServletRequest request) {
        String authHeader = request.getHeader("Authorization");
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            return ResponseEntity.status(401).body("인증 헤더가 유효하지 않습니다.");
        }
        String token = authHeader.substring(7);
        String email = jwtUtil.getUsernameFromToken(token);

        userService.logout(email);
        return ResponseEntity.ok("로그아웃이 완료되었습니다.");
    }
}
