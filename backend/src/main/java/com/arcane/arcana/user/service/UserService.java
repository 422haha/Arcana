package com.arcane.arcana.user.service;

import com.arcane.arcana.user.dto.RegisterDto;
import com.arcane.arcana.user.dto.UpdateDto;

/**
 * 사용자 관련 비즈니스 로직을 정의하는 서비스 인터페이스
 */
public interface UserService {

    void registerUser(RegisterDto registerDto);

    void verifyEmail(String email, String tokenOrCode);

    String login(String email, String rawPassword);

    void updateUser(Long userId, UpdateDto updateDto);

    void logout(String email);

    Long getUserIdByEmail(String email);
}
