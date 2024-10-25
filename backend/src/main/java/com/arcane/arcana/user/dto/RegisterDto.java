package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 회원 가입 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class RegisterDto {
    private String email;    // 사용자 이메일
    private String username; // 사용자 닉네임
    private String password; // 사용자 비밀번호
}
