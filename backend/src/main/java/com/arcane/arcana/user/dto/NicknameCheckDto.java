package com.arcane.arcana.user.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 닉네임 중복 확인 요청을 위한 DTO
 */
@Getter
@Setter
@NoArgsConstructor
public class NicknameCheckDto {

    private String nickname;
}
