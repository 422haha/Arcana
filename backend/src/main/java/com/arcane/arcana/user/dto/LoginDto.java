package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 로그인 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class LoginDto {
    private String email;
    private String password;
}
