package com.arcane.arcana.user.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 로그인 응답을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class LoginResponseDto {

    private String accessToken;
    private String refreshToken;
    private Long userId;
    private String nickname;
    private String language;
}
