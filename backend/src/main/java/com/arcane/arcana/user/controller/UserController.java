package com.arcane.arcana.user.controller;

import com.arcane.arcana.common.dto.ApiResponse;
import com.arcane.arcana.user.dto.*;
import com.arcane.arcana.user.service.UserService;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.exception.CustomException;
import io.swagger.v3.oas.annotations.Operation;
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

    @Operation(summary = "이메일 인증번호 전송", description = "사용자의 이메일로 인증번호를 전송합니다.")
    @ResponseBody
    @PostMapping("/verify-email")
    public ResponseEntity<ApiResponse<String>> sendAuthNumber(@RequestBody EmailDto emailDto) {
        userService.sendAuthNumber(emailDto.getEmail());
        return ResponseEntity.ok(new ApiResponse<>("인증번호가 이메일로 전송되었습니다.", null));
    }

    @Operation(summary = "인증번호 검증", description = "사용자가 입력한 인증번호를 검증합니다.")
    @ResponseBody
    @PostMapping("/register/authnumber")
    public ResponseEntity<ApiResponse<String>> verifyAuthNumber(
        @RequestBody AuthNumberDto authNumberDto) {
        userService.verifyAuthNumber(authNumberDto.getEmail(), authNumberDto.getAuthNumber());
        return ResponseEntity.ok(new ApiResponse<>("인증 완료되었습니다.", null));
    }

    @Operation(summary = "회원 가입", description = "사용자 정보를 입력받아 회원 가입을 완료합니다.")
    @ResponseBody
    @PostMapping("/register")
    public ResponseEntity<ApiResponse<String>> register(
        @Valid @RequestBody RegisterDto registerDto) {
        userService.registerUser(registerDto);
        return new ResponseEntity<>(new ApiResponse<>("가입 성공", null), HttpStatus.CREATED);
    }

    @Operation(summary = "사용자 정보 수정", description = "사용자의 정보를 수정합니다.")
    @ResponseBody
    @PutMapping("/update")
    public ResponseEntity<ApiResponse<String>> updateUser(@Valid @RequestBody UpdateDto updateDto,
        HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        Long userId = userService.getUserIdByEmail(email);

        userService.updateUser(userId, updateDto);
        return ResponseEntity.ok(new ApiResponse<>("사용자 정보가 수정되었습니다.", null));
    }

    @Operation(summary = "로그아웃", description = "사용자를 로그아웃합니다.")
    @ResponseBody
    @PostMapping("/logout")
    public ResponseEntity<ApiResponse<String>> logout(HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        String accessToken = extractTokenFromRequest(request);

        userService.logout(email, accessToken);
        return ResponseEntity.ok(new ApiResponse<>("로그아웃이 완료되었습니다.", null));
    }

    @Operation(summary = "언어 설정 저장", description = "사용자의 언어 설정을 저장합니다.")
    @ResponseBody
    @PostMapping("/language")
    public ResponseEntity<ApiResponse<LanguageDto>> saveLanguage(
        @Valid @RequestBody LanguageDto languageDto, HttpServletRequest request) {
        String email = jwtUtil.extractEmailFromRequest(request);
        userService.saveLanguage(email, languageDto.getLanguage());
        LanguageDto responseDto = new LanguageDto();
        responseDto.setLanguage(languageDto.getLanguage());
        return ResponseEntity.ok(new ApiResponse<>("언어 저장 성공", responseDto));
    }

    @Operation(summary = "닉네임 중복 체크", description = "닉네임의 사용 가능 여부를 확인합니다.")
    @ResponseBody
    @PostMapping("/check-nickname")
    public ResponseEntity<ApiResponse<Boolean>> checkNickname(
        @Valid @RequestBody NicknameCheckDto nicknameCheckDto) {
        boolean isAvailable = userService.isNicknameAvailable(nicknameCheckDto.getNickname());
        return ResponseEntity.ok(new ApiResponse<>("닉네임 사용 가능 여부 조회 성공", isAvailable));
    }

    @Operation(summary = "비밀번호 재설정 요청", description = "사용자 이메일로 비밀번호 재설정 이메일을 전송합니다.")
    @ResponseBody
    @PostMapping("/forgot-password")
    public ResponseEntity<ApiResponse<String>> forgotPassword(@RequestParam String email) {
        userService.sendPasswordResetEmail(email);
        return ResponseEntity.ok(new ApiResponse<>("비밀번호 재설정 이메일이 전송되었습니다.", null));
    }

    @Operation(summary = "비밀번호 재설정 페이지", description = "비밀번호 재설정을 위한 페이지를 표시합니다.")
    @GetMapping("/reset-password")
    public String showResetPasswordPage(@RequestParam String email, @RequestParam String token,
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

    @Operation(summary = "비밀번호 재설정", description = "새 비밀번호를 입력하여 비밀번호를 재설정합니다.")
    @PostMapping("/reset-password")
    public String resetPassword(@RequestParam String email, @RequestParam String token,
        @RequestParam String newPassword, Model model) {
        userService.resetPassword(email, token, newPassword);
        model.addAttribute("message", "비밀번호가 성공적으로 재설정되었습니다.");
        return "reset-password-confirm";
    }

    @Operation(summary = "로그인", description = "사용자 정보를 통해 로그인합니다.")
    @ResponseBody
    @PostMapping("/login")
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
