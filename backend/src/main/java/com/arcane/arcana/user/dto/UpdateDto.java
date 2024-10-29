package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 사용자 정보 수정을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class UpdateDto {
    private String nickname;    // 새로운 닉네임
    private String password;    // 새로운 비밀번호
    private String oldPassword; // 기존 비밀번호 (패스워드 변경 시 필요)
}
