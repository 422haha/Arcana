package com.arcane.arcana.user.dto;

import com.arcane.arcana.common.validation.PasswordConstraint;
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

    @PasswordConstraint
    private String newPassword;
}
