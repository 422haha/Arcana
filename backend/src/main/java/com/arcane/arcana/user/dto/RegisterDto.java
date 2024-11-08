package com.arcane.arcana.user.dto;

import com.arcane.arcana.common.validation.PasswordConstraint;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;

/**
 * 회원 가입 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class RegisterDto {

    @Email(message = "유효한 이메일 주소를 입력해주세요.")
    @NotBlank(message = "이메일은 필수 입력 사항입니다.")
    private String email;    // 사용자 이메일

    @NotBlank(message = "닉네임은 필수 입력 사항입니다.")
    private String nickname; // 사용자 닉네임

    @PasswordConstraint
    private String password; // 사용자 비밀번호
}
