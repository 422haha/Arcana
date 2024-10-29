package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 비밀번호 재설정 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class PasswordResetDto {

    private String email;
    private String token;
    private String newPassword;
}
