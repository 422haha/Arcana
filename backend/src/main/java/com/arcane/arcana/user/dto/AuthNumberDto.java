package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 인증번호 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class AuthNumberDto {

    private String email;
    private String authNumber;
}
