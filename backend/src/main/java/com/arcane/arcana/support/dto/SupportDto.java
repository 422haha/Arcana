package com.arcane.arcana.support.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.springframework.web.multipart.MultipartFile;

@Getter
@Setter
@NoArgsConstructor
public class SupportDto {

    private String category;
    private String email;
    private String title;
    private String description;
    private MultipartFile[] screenshots; // 스크린샷을 배열로 받아 처리
}
