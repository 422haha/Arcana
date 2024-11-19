package com.arcane.arcana.user.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.dto.RegisterDto;
import com.arcane.arcana.user.dto.UpdateDto;
import com.arcane.arcana.user.dto.LoginDto;
import com.arcane.arcana.user.dto.LoginResponseDto;

/**
 * 사용자 관련 비즈니스 로직을 정의하는 서비스 인터페이스
 */
public interface UserService {

    void registerUser(RegisterDto registerDto);

    void updateUser(Long userId, UpdateDto updateDto);

    void logout(String email, String accessToken);

    Long getUserIdByEmail(String email);

    void updateRefreshToken(String email, String newRefreshToken);

    String getStoredRefreshToken(String email);

    void saveLanguage(String email, String language);

    boolean isNicknameAvailable(String nickname);

    void sendPasswordResetEmail(String email);

    void resetPassword(String email, String token, String newPassword);

    boolean isResetTokenValid(String email, String token);

    User getUserByEmail(String email);

    LoginResponseDto login(LoginDto loginDto);

    void verifyAuthNumber(String email, String authNumber);

    void sendAuthNumber(String email);

    void withdrawUser(Long userId);

    void restoreUser(String email, String restoreToken);
}
