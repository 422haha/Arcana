package com.arcane.arcana.user.controller;

import com.arcane.arcana.common.dto.ApiResponse;
import com.arcane.arcana.user.dto.*;
import com.arcane.arcana.user.service.UserService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.http.HttpStatus;
import jakarta.validation.Valid;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

/**
 * 사용자 관련 요청을 처리하는 컨트롤러
 */
@Controller
@RequestMapping("/user")
public class UserController {

    private final UserService userService;
    private final JwtUtil jwtUtil;

    public UserController(UserService userService, JwtUtil jwtUtil) {
        this.userService = userService;
        this.jwtUtil = jwtUtil;
    }

    @PostMapping("/verify-email")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> sendAuthNumber(@RequestBody EmailDto emailDto) {
        userService.sendAuthNumber(emailDto.getEmail());
        return ResponseEntity.ok(new ApiResponse<>("인증번호가 이메일로 전송되었습니다.", null));
    }

    @PostMapping("/register/authnumber")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> verifyAuthNumber(
        @RequestBody AuthNumberDto authNumberDto) {
        userService.verifyAuthNumber(authNumberDto.getEmail(), authNumberDto.getAuthNumber());
        return ResponseEntity.ok(new ApiResponse<>("인증 완료되었습니다.", null));
    }

    @PostMapping("/register")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> register(
        @Valid @RequestBody RegisterDto registerDto) {
        userService.registerUser(registerDto);
        return ResponseEntity.ok(new ApiResponse<>("가입 성공", null));
    }

    @PutMapping("/update")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> updateUser(@Valid @RequestBody UpdateDto updateDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        Long userId = userService.getUserIdByEmail(email);

        userService.updateUser(userId, updateDto);
        return ResponseEntity.ok(new ApiResponse<>("사용자 정보가 수정되었습니다.", null));
    }

    @PostMapping("/logout")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> logout(HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        String accessToken = extractTokenFromRequest(request);

        userService.logout(email, accessToken);
        return ResponseEntity.ok(new ApiResponse<>("로그아웃이 완료되었습니다.", null));
    }

    @PostMapping("/language")
    @ResponseBody
    public ResponseEntity<ApiResponse<LanguageDto>> saveLanguage(
        @Valid @RequestBody LanguageDto languageDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        userService.saveLanguage(email, languageDto.getLanguage());
        LanguageDto responseDto = new LanguageDto();
        responseDto.setLanguage(languageDto.getLanguage());
        return ResponseEntity.ok(new ApiResponse<>("언어 저장 성공", responseDto));
    }

    @PostMapping("/check-nickname")
    @ResponseBody
    public ResponseEntity<ApiResponse<Boolean>> checkNickname(
        @Valid @RequestBody NicknameCheckDto nicknameCheckDto) {
        boolean isAvailable = userService.isNicknameAvailable(nicknameCheckDto.getNickname());
        return ResponseEntity.ok(new ApiResponse<>("닉네임 사용 가능 여부 조회 성공", isAvailable));
    }

    @PostMapping("/forgot-password")
    @ResponseBody
    public ResponseEntity<ApiResponse<String>> forgotPassword(@RequestParam String email) {
        userService.sendPasswordResetEmail(email);
        return ResponseEntity.ok(new ApiResponse<>("비밀번호 재설정 이메일이 전송되었습니다.", null));
    }

    @GetMapping("/reset-password")
    public String showResetPasswordPage(@RequestParam String email,
        @RequestParam String token,
        Model model) {
        boolean isValid = userService.isResetTokenValid(email, token);
        if (isValid) {
            model.addAttribute("email", email);
            model.addAttribute("token", token);
            return "reset-password-form";
        } else {
            throw new CustomException("유효하지 않은 토큰입니다.", HttpStatus.BAD_REQUEST);
        }
    }

    @PostMapping("/reset-password")
    public String resetPassword(@RequestParam String email,
        @RequestParam String token,
        @RequestParam String newPassword,
        Model model) {
        userService.resetPassword(email, token, newPassword);
        model.addAttribute("message", "비밀번호가 성공적으로 재설정되었습니다.");
        return "reset-password-confirm";
    }

    @PostMapping("/login")
    @ResponseBody
    public ResponseEntity<ApiResponse<LoginResponseDto>> login(
        @Valid @RequestBody LoginDto loginDto) {
        LoginResponseDto responseDto = userService.login(loginDto);
        return ResponseEntity.ok(new ApiResponse<>("로그인 성공", responseDto));
    }

    private String extractTokenFromRequest(HttpServletRequest request) {
        String authHeader = request.getHeader("Authorization");
        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            throw new CustomException("인증 헤더가 유효하지 않습니다.", HttpStatus.UNAUTHORIZED);
        }
        return authHeader.substring(7);
    }
}
